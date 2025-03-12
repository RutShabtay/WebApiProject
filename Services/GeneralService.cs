using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApiProject.Models;

namespace WebApiProject.Services
{
    public static class GeneralService
    {
        //Function to read from a Json file
        public static List<Object> ReadFromJsonFile(string jsonFilePath, string kind)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"The file {jsonFilePath} was not found.");
            }

            string jsonContetnt = File.ReadAllText(jsonFilePath);
            if (kind.Equals("user"))
            {
                var list = JsonConvert.DeserializeObject<List<User>>(jsonContetnt);
                return list.Cast<Object>().ToList();
            }
            else if (kind.Equals("job"))
            {
                var list = JsonConvert.DeserializeObject<List<Job>>(jsonContetnt);
                return list.Cast<Object>().ToList();
            }
            else
            {
                throw new ArgumentException("Invalid kind specified. Expected 'user' or 'job'.");
            }
        }

        //Function to write for a Json File
        public static ActionResult WriteToJsonFile(string jsonFilePath, Object newObject, string type)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"The file {jsonFilePath} was not found.");
            }
            var jsonContetnt = ReadFromJsonFile(jsonFilePath, type);

            jsonContetnt.Add(newObject);

            string jsonContent = JsonConvert.SerializeObject(jsonContetnt, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonContent);
            return new ObjectResult("DB update successfully.") { StatusCode = 201 };
        }

        public static string GetCurrentToken(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring(7);
                return token;
            }
            return null;
        }

        public static IActionResult GetUserPasswordFromToken(HttpContext httpContext)
        {
            var currentToken = GetCurrentToken(httpContext);
            if (currentToken == null)
                return new ObjectResult("Unauthorized: Invalid or missing token") { StatusCode = 401 };
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(currentToken) as JwtSecurityToken;
            var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "password")?.Value;
            System.Console.WriteLine(userIdClaim);
            if (string.IsNullOrEmpty(userIdClaim))
                return new ObjectResult("User Password not found in token") { StatusCode = 400 };

            return new ObjectResult(userIdClaim) { StatusCode = 200 };
        }

        //get userRole from token by his claim.
        public static IActionResult GetUserTypeFromToken(HttpContext httpContext)
        {
            var currentToken = GetCurrentToken(httpContext);
            if (currentToken == null)
                return new ObjectResult("Unauthorized: Invalid or missing token") { StatusCode = 401 };
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(currentToken) as JwtSecurityToken;
            var userTypeClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "type")?.Value;

            if (string.IsNullOrEmpty(userTypeClaim))
                return new ObjectResult("User ID not found in token") { StatusCode = 400 };

            return new ObjectResult(userTypeClaim) { StatusCode = 200 };
        }

    }
}


