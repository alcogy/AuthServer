module DB

open Npgsql.FSharp

let connectionString: string = "Host=localhost; Database=postgres; Username=root; Password=root;"

let AuthenticateUser (email: string) (password: string): bool =
    try
        connectionString
        |> Sql.connect
        |> Sql.query "SELECT COUNT(*) as count FROM users WHERE email = @email AND password = @password"
        |> Sql.parameters [ "@email", Sql.string email; "@password", Sql.string password ]
        |> Sql.executeRow (fun read -> read.int "count")
        |> fun count -> count > 0
    
    with
        | ex ->
            printfn "%s" ex.Message
            false
