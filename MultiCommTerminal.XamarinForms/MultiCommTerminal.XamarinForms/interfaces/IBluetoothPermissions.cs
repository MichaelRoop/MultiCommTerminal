using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MultiCommTerminal.XamarinForms.interfaces {

    /// <summary>
    /// To use to register as dependency service for different platforms
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/xamarin/essentials/permissions?tabs=android
    /// </remarks>
    public interface IBluetoothPermissions {

        /// <summary>Used to check existing permission status for Location in use</summary>
        /// <returns></returns>
        Task<PermissionStatus> CheckStatusAsync();

        /// <summary>Used to request permission for Location in use</summary>
        /// <returns></returns>
        Task<PermissionStatus> RequestAsync();

    }
}
