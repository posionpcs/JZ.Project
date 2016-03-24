using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Framework.DAL.SqlServer;
using FrameWork.DAL;
using Autofac;
using FrameWork;
using FrameWork.ESB;
using FrameWork.NoSql;
using FrameWork.Utils;
using JZ.Manage;
using FrameWork.AutoFac;

namespace JZ.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var container=AutoFacHelper.GetContainner().Build();
            var list= container.Resolve<IRepository<Users>>().ToList(c=>true);




            Console.WriteLine(list.Count);




        }
    }
}
