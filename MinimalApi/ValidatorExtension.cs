using FluentValidation;

namespace MinimalApi
{
    public static class ValidatorExtension
    {
        public static RouteHandlerBuilder WithValidator<T>(this RouteHandlerBuilder builder) // Metoda rozszerzajaca
            where T : class
        {
            builder.Add(endpointsBuilder =>
            {
                var originalDelegate = endpointsBuilder.RequestDelegate;
                endpointsBuilder.RequestDelegate = async httpContext =>
                {
                    var validator = httpContext.RequestServices.GetRequiredService<IValidator<T>>();

                    httpContext.Request.EnableBuffering();
                    var body = await httpContext.Request.ReadFromJsonAsync<T>();

                    if(body != null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await httpContext.Response.WriteAsync("Couldn't map body to request model");
                        return;
                    }

                    var validationResult = validator.Validate(body);
                    if(!validationResult.IsValid)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await httpContext.Response.WriteAsJsonAsync(validationResult.Errors);
                        return;
                    }
                     
                    httpContext.Request.Body.Position = 0; // Przesuniecie na sam poczatek

                    await originalDelegate(httpContext);
                };
            });

            return builder;
        }
    }
}
