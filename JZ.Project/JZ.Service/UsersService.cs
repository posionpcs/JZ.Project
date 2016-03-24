using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameWork.DAL;
using JZ.Manage;

namespace JZ.Service
{
    public class UsersService
    {
        private IRepository<Users> userRepository;
        public UsersService(IRepository<Users> userRepository)
        {
            this.userRepository = userRepository;
        }


        public List<Users> GetList()
        {
            return userRepository.ToList(c => true);
        }
    }
}
