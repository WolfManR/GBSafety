using CRUD_Cards_webapi.Models;

using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services
{
    internal interface IDebetCardsService
    {
        Result<int> Create(CreateDebetCardRequest cardData);
        void Delete(int id);
        IEnumerable<DebetCardResponse> Get();
        Result<DebetCardResponse> Get(int id);
        Result Update(int id, UpdateDebetCardRequest cardData);
    }
}