using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace MinimalApi.ToDo
{
    public static class ToDoRequests
    {
        // Extension Method
        public static WebApplication RegisterEndpoints(this WebApplication app)
        {
            app.MapGet("/todos", ToDoRequests.GetAll)
                .Produces<List<ToDo>>() // () domyślny kod 200
                .WithTags("To Dos") // Zmiana nazwy w Swaggerze
                .RequireAuthorization();

            app.MapGet("/todos/{id}", ToDoRequests.GetById)
                .Produces<ToDo>()
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("To Dos")
                .AllowAnonymous();

            app.MapPost("/todos", ToDoRequests.Create)
                .Produces<ToDo>(StatusCodes.Status201Created)
                .Accepts<ToDo>("application/json")
                .WithTags("To Dos")
                .WithValidator<ToDo>();

            app.MapPut("/todos/{id}", ToDoRequests.Update)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Accepts<ToDo>("application/json")
                .WithTags("To Dos")
                .WithValidator<ToDo>();

            app.MapDelete("/todos/{id}", ToDoRequests.Delete)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("To Dos")
                .ExcludeFromDescription(); // Ukrywanie mapDelete z publicznej definicji Swaggera

            return app;
        }

        public static IResult GetAll(IToDoService service)
        {
            var todos = service.GetAll();
            return Results.Ok(todos);
        }

        [AllowAnonymous]
        public static IResult GetById(IToDoService service, Guid id)
        {
            var todo = service.GetById(id);
            if(todo is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(todo);
        }

        [Authorize]
        public static IResult Create(IToDoService service, ToDo toDo)
        {
            service.Create(toDo);
            return Results.Created($"/todos/{toDo.Id}", toDo);
        }

        public static IResult Update(IToDoService service, Guid id, ToDo toDo)
        {
            var todo = service.GetById(id);
            if (todo is null)
            {
                return Results.NotFound();
            }

            service.Update(toDo);

            return Results.NoContent();
        }

        public static IResult Delete(IToDoService service, Guid id)
        {
            var todo = service.GetById(id);
            if (todo is null)
            {
                return Results.NotFound();
            }

            service.Delete(id);
            return Results.NoContent();
        }
    }
}
