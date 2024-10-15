namespace Imsat.Bashmaky.Web.WebApi.Dto
{
    public record class AddUserRequest(string Login, string Password, string? Name);
    public record class SignInRequest(string Login, string Password); 
}
