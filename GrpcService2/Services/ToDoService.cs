using Grpc.Core;
using GrpcService1.Data;
using GrpcService2.Protos;
using Microsoft.EntityFrameworkCore;

namespace GrpcService2.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext _AppDbContext;

        public ToDoService(AppDbContext appDbContext)
        {
            _AppDbContext = appDbContext;
        }

        public override async Task<createToDoResoponse> CreateToDo(createToDo request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty)

                throw new RpcException(new Status(StatusCode.InvalidArgument, "You Must Pass Valid Object"));

            var ToDo = new GrpcService1.Models.ToDoItem()
            {
                Description = request.Description,
                Title = request.Title,
            };
            await _AppDbContext.toDoItems.AddAsync(ToDo);
            await _AppDbContext.SaveChangesAsync();

            return await Task.FromResult(new createToDoResoponse()
            {
                Id = ToDo.Id
            });
        }

        public override async Task<Updatetodoresponse> UpdateToDo(Updatetodorequest request, ServerCallContext context)
        {
            var Item = await _AppDbContext.toDoItems.FirstOrDefaultAsync(c => c.Id == request.Id);

            if (Item is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Object with id {request.Id} is not found"));

            Item.status = request.Status;
            Item.Title = request.Title;
            Item.Description = request.Description;

            _AppDbContext.toDoItems.Update(Item);
            await _AppDbContext.SaveChangesAsync();

            return new Updatetodoresponse()
            {
                Id = Item.Id
            };
        }

        public override async Task<readToDoResponse> GetById(readToDoRequest request, ServerCallContext context)
        {
            var result = await _AppDbContext.toDoItems.FirstOrDefaultAsync(c => c.Id == request.Id);

            if (result is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Object with id {request.Id} is not found"));

            return await Task.FromResult(new readToDoResponse()
            {
                Id = result.Id,
                Description = result.Description,
                Status = result.status,
                Title = result.Title
            });
        }

        public override async Task<getallresponse> GetAll(getallrequest request, ServerCallContext context)
        {
            var result = await _AppDbContext.toDoItems.ToListAsync();
            var Response = new getallresponse();

            foreach (var item in result)
                Response.ToDo.Add(new readToDoResponse()
                {
                    Id = item.Id,
                    Description = item.Description,
                    Status = item.status,
                    Title = item.Title
                });

            return Response;
        }

        public override async Task<deletetodoresponse> DeleteToDo(deletetodorequest request, ServerCallContext context)
        {
            var Item = await _AppDbContext.toDoItems.FirstOrDefaultAsync(c => c.Id == request.Id);

            if (Item is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Object with id {request.Id} is not found"));

            _AppDbContext.toDoItems.Remove(Item);
            await _AppDbContext.SaveChangesAsync();

            return new deletetodoresponse()
            {
                Id = request.Id,
            };
        }
    }
}