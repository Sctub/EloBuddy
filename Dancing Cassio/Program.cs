using EloBuddy.SDK.Events;

namespace Dancing_Cassio
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += eventArgs => Cassiopeia.Loading_OnLoadingComplete();
        }
    }
}