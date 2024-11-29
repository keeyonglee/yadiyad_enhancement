using Nop.Plugin.NopStation.Core.Infrastructure;

namespace Nop.Plugin.NopStation.Core.Services
{
    public interface INopStationLicenseService
    {
		bool IsLicensed();

        KeyVerificationResult VerifyProductKey(string key);
    }

    public class ByPassNopStationLicense : INopStationLicenseService
    {
        public bool IsLicensed() => true;

        public KeyVerificationResult VerifyProductKey(string key) => KeyVerificationResult.Valid;
    }
}
