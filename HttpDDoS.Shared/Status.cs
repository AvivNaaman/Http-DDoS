using System;

namespace HttpDDoS.Shared
{
    public class Status
    {
        public string Url { get; set; }

        public bool ShouldRun => Url is not null or "";
    }
}
