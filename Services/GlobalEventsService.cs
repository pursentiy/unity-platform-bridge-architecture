using Plugins.FSignal;

namespace Services.BridgeRelatedServices
{
    public class GlobalEventsService
    {
        public FSignal<bool> OnGameOverlayStartedSignal = new FSignal<bool>();
        
    }
}