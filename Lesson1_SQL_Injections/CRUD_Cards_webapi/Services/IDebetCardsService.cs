using CRUD_Cards_webapi.Models;

using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services
{
    internal interface IDebetCardsService
    {
        Task<Result<int>> Create(CreateDebetCardRequest cardData);
        Task Delete(int id);
        Task<IEnumerable<DebetCardResponse>> Get();
        Task<Result<DebetCardResponse>> Get(int id);
        Task<Result> Update(int id, UpdateDebetCardRequest cardData);
    }
}