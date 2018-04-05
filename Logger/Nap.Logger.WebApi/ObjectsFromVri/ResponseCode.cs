namespace Nap.Logger.WebApi.ObjectsFromVri
{
    public enum ResponseCode
    {
        None = 0,
        /// <summary>
        /// 2XX Codes: Success codes. Tells the client that the request succeeded.
        /// </summary>
        Ok = 200,
        Created = 201,
        NoContent = 204,

        /// <summary>
        /// 3XX Codes: Redirect codes. Tells the client that they may need to 
        /// redirect to another location.
        /// </summary>
        NotModified = 304,

        /// <summary>
        /// 4XX Codes: Client Error codes. Tells the client that something was wrong 
        /// with what it sent to the server.
        /// </summary>
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,//use 410 if resource is known to be permanently gone
        Conflict = 409,
        Gone = 410,

        /// <summary>
        /// 5XX Codes: Server Error codes. Tells the client that something went wrong on the server's side, 
        /// so that the client may attempt the request again, possibly at a later time.
        /// </summary>
        InternalServerError = 500
    }
}