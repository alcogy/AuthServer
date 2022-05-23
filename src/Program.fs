module Program

open System
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open Saturn
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.IdentityModel.Tokens

let secret: string = "spadR2dre#u-ruBrE@TepA&*Uf@U"
let issuer: string = "Alcogy"

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("*")
        .AllowAnyMethod()
        .AllowAnyHeader()
        |> ignore

let generateToken (email: string) =
    [|
        Claim(JwtRegisteredClaimNames.Sub, email);
        Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    |]
    |> Auth.generateJWT (secret, SecurityAlgorithms.HmacSha256) issuer (DateTime.UtcNow.AddHours(1.0))

let authenticate email password = 
    DB.AuthenticateUser email (Domain.Encryption password)

let HandlePostToken =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! param = ctx.BindJsonAsync<Domain.User>()
            if authenticate param.email param.password then
                let tokenResult = generateToken param.email
                return! json tokenResult next ctx
            else
                return! json null next ctx
        }

let appRouter = router {
    not_found_handler (setStatusCode 404 >=> text "Not Found")
    post "/auth" HandlePostToken
    get "/" (text "Hello Auth Server")
}

let app = application {
    use_jwt_authentication secret issuer
    use_router appRouter
    use_cors "CORS_policy" configureCors
    memory_cache
}

run app 