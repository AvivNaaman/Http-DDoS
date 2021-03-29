using HttpDDoS.Shared;

namespace HttpDDoS.Master
{
    public interface IStatusService
    {
        Status Status { get; }

        void SetStatus(Status newStatus);
    }
}