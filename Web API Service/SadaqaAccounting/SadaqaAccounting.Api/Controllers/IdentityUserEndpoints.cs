namespace SadaqaAccounting.Api.Controllers
{
    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            // Extend sign up method
            app.MapPost("/signup", CreateUser);

            // Extend sign in method
            app.MapPost("/signin", SignIn);

            return app;
        }

        [AllowAnonymous]
        private static async Task<IResult> CreateUser(UserManager<User> userManager, [FromBody] UserRegistrationModel userRegistrationModel)
        {
            var user = new User
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName
            };

            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

            if (result.Succeeded)
                return Results.Ok(result);

            return Results.BadRequest(result);
        }

        [AllowAnonymous]
        private static async Task<IResult> SignIn(UserManager<User> userManager, [FromBody] Application.ApplicationLogic.IdentityLogic.Model.LoginModel loginModel, IOptions<AppSettings> appSeeting)
        {
            var existUser = await userManager.FindByEmailAsync(loginModel.Email);

            if (existUser is not null && await userManager.CheckPasswordAsync(existUser, loginModel.Password))
            {
                var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSeeting.Value.JWTSecret));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, existUser.Id.ToString())
                    }),

                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return Results.Ok(new { token });
            }

            return Results.BadRequest(new { message = "Username or password is incorrect." });
        }
    }
}