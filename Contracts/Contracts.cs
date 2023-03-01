using System.Net;
using OneOf;
using OneOf.Types;

namespace Contracts;

public interface IRequest
{
    HttpStatusCode HttpStatusCode { get; }
}

public interface IGetRequest<TParentIdentity, TIdentity, TView> where TParentIdentity : struct where TIdentity : struct where TView : Enum
{
    TParentIdentity ParentId { get; }
    TIdentity Id { get; }
    TView View { get; }
}

public interface ICreateRequest<TParentIdentity, out TBody> where TParentIdentity : struct
{
    TParentIdentity ParentId { get; }
    TBody Body { get; }
}

public interface IUpdateRequest<TParentIdentity, TIdentity, out TBody> where TParentIdentity : struct where TIdentity : struct
{
    TIdentity Id { get; }
    TParentIdentity ParentId { get; }
    TBody Body { get; }
}

public interface IDeleteRequest<TParentIdentity, TIdentity> where TParentIdentity : struct where TIdentity : struct
{
    TParentIdentity ParentId { get; }
    TIdentity Id { get; }
}

public interface IBatchGetRequest<TParentIdentity, TIdentity> where TParentIdentity : struct where TIdentity : struct
{
    TParentIdentity ParentId { get; }
    TIdentity[] Identities { get; }
}

public interface IListRequest<TParentIdentity, TView> where TParentIdentity : struct where TView : Enum
{
    TParentIdentity ParentId { get; }
    int PageSize { get; }
    string PageToken { get; }
    TView? View { get; }
}

public interface IListResponse<out TListItem>
{
    public TListItem[] Items { get; }

    string NextPageToken { get; }
    // string PreviousPageToken { get; }
}

public interface ICreate<TParentIdentity, in TBody, TIdentity> where TParentIdentity : struct
{
    public Task<TIdentity> Create(ICreateRequest<TParentIdentity, TBody> request, CancellationToken cancellationToken);
}

public interface IUpdate<TParentIdentity, TIdentity, in TBody> where TParentIdentity : struct where TIdentity : struct
{
    public Task Update(IUpdateRequest<TParentIdentity, TIdentity, TBody> request, CancellationToken cancellationToken);
}

public interface IDelete<TParentIdentity, TIdentity, TEntity> where TParentIdentity : struct where TIdentity : struct
{
    public Task<OneOf<Success, NotFound>> Delete(IDeleteRequest<TParentIdentity, TIdentity> request, CancellationToken cancellationToken);
}

public interface IList<TParentIdentity, TView, TListItem> where TParentIdentity : struct where TView : Enum
{
    public Task<OneOf<IListResponse<TListItem>, Error<Exception>>> List(IListRequest<TParentIdentity, TView> request, CancellationToken cancellationToken);
}

public interface IGet<TParentIdentity, TIdentity, TView, TResponse> where TParentIdentity : struct where TIdentity : struct where TView : Enum
{
    public Task<OneOf<TResponse, NotFound, Error<Exception>>> Get(IGetRequest<TParentIdentity, TIdentity, TView> request, CancellationToken cancellationToken);
}

public interface ITransform<TSelf, out TEntity> where TSelf : ITransform<TSelf, TEntity>
{
    public TEntity Transform();
}

public record ListResponse<TResponse>(TResponse[] Items, string NextPageToken) : IListResponse<TResponse>;

// public interface IValidateRequestBody<out TReturn>
// {
//     public static abstract TReturn Validate();
// }