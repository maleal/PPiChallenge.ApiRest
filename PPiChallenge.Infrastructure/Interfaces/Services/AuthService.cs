using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PPiChallenge.Core.DTOs;
using PPiChallenge.Core.Entities;
using PPiChallenge.Core.Interfaces;
using PPiChallenge.Core.Interfaces.Services;
using PPiChallenge.Infrastructure.DataBaseIttion;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Infrastructure.Interfaces.Services
{
    public class AuthService : IAuthService
    {

        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _config = config;
        }

        public async Task<LoginResDto?> LoginAsync(LoginReqDto dto)
        {
            try
            {
                _logger.LogInformation($"Intentando login para usuario:'{dto.Usuario}'");

                //cuenta por usuario ingresado
                var cuenta = await _unitOfWork.Cuentas.GetByUsuarioAsync(dto.Usuario);
                if (cuenta == null)
                {
                    _logger.LogWarning($"Usuario:'{dto.Usuario}' no encontrado");
                    throw new KeyNotFoundException($"Usuario:'{dto.Usuario}' no encontrado."); //404
                }
                //Si esta habilitado! (forbidden man)
                if (!cuenta.IsEnabled)
                {
                    _logger.LogWarning($"Usuario:'{dto.Usuario}' esta deshabilitado");
                    throw new UnauthorizedAccessException($"Usuario '{dto.Usuario}' deshabilitado."); //403
                }

                //Validamos la password con BCrypt (usamos lib. 'BCrypt.Net-Next' ver NuGet Instalados)
                try
                {
                    if (!BCrypt.Net.BCrypt.Verify(dto.Password, cuenta.PasswordHash))
                    {
                        _logger.LogWarning($"Contraseña inválida para {dto.Usuario}");
                        throw new ArgumentException("Credenciales incorrectas."); //401
                    }
                }
                catch (BCrypt.Net.SaltParseException ex)
                {
                    _logger.LogWarning(ex, $"Hash invalido en DB para usuario '{dto.Usuario}'");
                    throw new ArgumentException("Credenciales incorrectas."); //fuerza 401
                }
                

                //Crear claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, cuenta.Usuario),
                    new Claim(JwtRegisteredClaimNames.Sub, cuenta.Usuario),
                    new Claim("idCuenta", cuenta.IdCuenta.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                //Generar clave
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiration = DateTime.UtcNow.AddHours(1);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds
                );

                _logger.LogInformation($"Login OK para usuario:{dto.Usuario}");

                return new LoginResDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expiration
                };

            }
            catch(Exception ex)
            {
                //relanzamos para el middleware !!
                _logger.LogError(ex, $"Error en LoginAsync usuario:'{dto.Usuario}'");
                throw;
            }

        }

        public async Task<CuentaDto> RegistrarCuentaAsync(RegistroCuentaDto dto)
        {
            try
            {
                _logger.LogInformation($"Intentando registrar usuario:'{dto.Usuario}' Email: '{dto.Email}'");

                //duplicado el Email?
                var existEmail = await _unitOfWork.Cuentas.GetByEmailAsync(dto.Email);
                if (existEmail != null)
                {
                    _logger.LogWarning($"Email ya registrado: {dto.Email}");
                    throw new InvalidOperationException("Email ya registrado."); //400
                }

                //duplicado el Usuario?
                var existUsuario = await _unitOfWork.Cuentas.GetByUsuarioAsync(dto.Usuario);
                if (existUsuario != null)
                {
                    _logger.LogWarning($"Usuario ya registrado: {dto.Usuario}");
                    throw new InvalidOperationException("Usuario ya registrado."); //400
                }

                //Hashear contraseña
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                //Crear entidad
                var cuenta = new Cuenta
                {
                    Usuario = dto.Usuario,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    Saldo = 0,
                    Moneda = "ARS",
                    IsEnabled = true
                };

                //Guardar
                await _unitOfWork.Cuentas.AddAsync(cuenta);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Usuario registrado correctamente:'{dto.Usuario}'");

                //Return DTO sin password
                return new CuentaDto
                {
                    IdCuenta = cuenta.IdCuenta,
                    Usuario = cuenta.Usuario,
                    Email = cuenta.Email,
                    Saldo = cuenta.Saldo,
                    Moneda = cuenta.Moneda,
                    IsEnabled = cuenta.IsEnabled
                };
            }
            catch (Exception ex) when (ex is InvalidOperationException)
            {
                throw; //al middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registrando usuario:'{dto.Usuario}'");
                throw;
            }
        }


    }
}
