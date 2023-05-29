using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IAuthRepository
    {
        public Member CheckUserInDatabase(LoginModel loginModel);
    }
}
