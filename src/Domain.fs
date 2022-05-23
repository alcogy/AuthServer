module Domain

open System.Security.Cryptography
open System.Text

[<CLIMutable>]
type User = {
    email: string
    password: string
}

[<CLIMutable>]
type TokenResult = {
    token: string
}

let Encryption (password: string) = 
    use md5 = MD5.Create()
    (StringBuilder(), md5.ComputeHash(Encoding.UTF8.GetBytes(password)))
    ||> Array.fold (fun sb b -> sb.Append(b.ToString("x2")))
    |> string