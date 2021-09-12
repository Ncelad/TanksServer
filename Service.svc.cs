using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService
    {
        string connectionStr = "Data Source=SQL5061.site4now.net;Initial Catalog=db_a7966a_ion;User Id=db_a7966a_ion_admin;Password=Gorb_bc24";


        public string Register(string email, string password, string password_check)
        { 
            try
            {
                if (!email.Contains("@gmail.com"))
                {
                    throw new Exception("Incorrect email!");
                }
                if(password != password_check)
                {
                    throw new Exception("Passwords not same!");
                }
                User new_user = new User(email, password);
                using (SqlConnection conn = new SqlConnection(connectionStr))
                {
                    conn.Open();
                    using(var transact = conn.BeginTransaction())
                    {
                        try
                        {
                            conn.Query("Registrate", new {new_user.Email, new_user.Password}, transact, true, null, CommandType.StoredProcedure);
                            transact.Commit();
                        }
                        catch (Exception)
                        {
                            transact.Rollback();
                            return "Transaction failed!";
                        }
                    }
                }
                return "Registration success!";
            }
            catch (Exception e)
            {

                return e.Message + "Try again!";
            }
        }

        public string Login(string email, string password)
        {
            try
            {
                if (!email.Contains("@gmail.com"))
                {
                    throw new Exception("Incorrect email!");
                }
                User new_user = new User(email, password);
                User logined_user;
                using (SqlConnection conn = new SqlConnection(connectionStr))
                {
                    conn.Open();
                    using (var transact = conn.BeginTransaction())
                    {
                        try
                        {
                            List<User> result = conn.Query<User>("Logine", new { new_user.Email, new_user.Password }, transact, true, null, CommandType.StoredProcedure).ToList();

                            if (result.Count > 0)
                            {
                                logined_user = result[0];
                            }
                            else
                            {
                                throw new Exception("Wrong email or password!");
                            }
                            transact.Commit();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != "")
                            {
                                transact.Rollback();
                                return ex.Message;
                            }
                            else
                            {
                                transact.Rollback();
                                return "Transaction failed!";
                            }
                        }
                    }
                }
                return "Login success!";
            }
            catch (Exception e)
            {

                return e.Message + "Try again!";
            }
        }
    }
}
