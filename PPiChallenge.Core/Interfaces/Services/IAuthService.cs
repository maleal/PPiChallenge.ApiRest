using PPiChallenge.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResDto?> LoginAsync(LoginReqDto dto);
        Task<CuentaDto> RegistrarCuentaAsync(RegistroCuentaDto dto);
    }
}
