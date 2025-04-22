using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace grupofunctionsmartes
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string idempleado = req.Query["idempleado"];
            if (idempleado == null)
            {
                return new BadRequestObjectResult
                    ("Debe incluir un Id de empleado");
            }
            else
            {
                string connectionString =
                    Environment.GetEnvironmentVariable("SqlAzure");
                SqlConnection cn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand();
                com.Connection = cn;
                com.CommandType = System.Data.CommandType.Text;
                com.CommandText = "update EMP set SALARIO=SALARIO+1" +
                    " where EMP_NO = " + idempleado;
                cn.Open();
                com.ExecuteNonQuery();
                com.CommandText = "select * from EMP where EMP_NO="
                    + idempleado;
                SqlDataReader reader = com.ExecuteReader();
                reader.Read();
                string mensaje = "El empleado " +
                    reader["APELLIDO"] + " ha incrementado su salario" +
                    " a " + reader["SALARIO"];
                cn.Close();
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult(mensaje);
            }
        }
    }
}
