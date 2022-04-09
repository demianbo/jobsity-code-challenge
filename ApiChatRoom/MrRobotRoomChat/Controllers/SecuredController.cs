using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrRobotRoomChat.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [TokenableAuthorize]
    public class SecuredController : ControllerBase
    {
        public string Token { get; set; }
        public RoomSession Session { get; set; }
    }

    public class TokenableAuthorize : ActionFilterAttribute
    {

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var endpoint = context.HttpContext.GetEndpoint();

            SecuredController tokenController = context.Controller as SecuredController;

            if (tokenController != null)
            {
                var authorization = tokenController.Request.Headers["Authorization"];

                if (!string.IsNullOrEmpty(authorization.ToString()))
                {
                    tokenController.Token = authorization;
                    try
                    {
                        tokenController.Session = new RoomSession(tokenController.Token);

                        await next.Invoke();
                    }
                    catch (Exception ee)
                    {
                        context.HttpContext.Response.Headers.Add("AuthenticationStatus", "NotAuthorized");
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Missing or invalid token. " + ee.Message;
                    }
                }
                else
                {
                    if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
                    {
                        await next.Invoke();
                        return;
                    }

                    context.HttpContext.Response.Headers.Add("AuthenticationStatus", "NotAuthorized");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Missing or invalid token. ";

                }
            }
            else
            {
                throw new Exception("TokenableAuthorize must be used only in a SecuredController inheritance");
            }

        }

    }

    public class RoomSession
    {
        public RoomSession(string token)
        {
            _token = token;
            ProcessToken();
        }

        private string _token;
        public DateTime FechaExpiracion { get; set; }
        public string Id { get; set; }
        public string Room { get; set; }

        private void ProcessToken()
        {
            /* DECODE JWT AND READ CLAIMS */
            _token = _token.Replace("bearer ", "").Replace("Bearer ", "").Replace("BEARER ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(_token);
            var tokenS = handler.ReadToken(_token) as JwtSecurityToken;

            Id = tokenS.Claims.First(claim => claim.Type == "id").Value;
            FechaExpiracion = jsonToken.ValidTo;
        }
    }
}