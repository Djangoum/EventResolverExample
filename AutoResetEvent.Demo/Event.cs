using System.Threading;

namespace ResetEventAuto.Demo
{
    public class Event<TDependencyKey>
    {
        internal Event()
        {

        }

        public TDependencyKey Id { get; set; }
        public TDependencyKey[] Dependencies { get; set; }
    }
}
