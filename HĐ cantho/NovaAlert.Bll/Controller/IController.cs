using System;
using System.Collections.Generic;
using System.ComponentModel;
using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// IController is the basic interface for all controllers.
    /// </summary>
    public interface IController : INovaAlertServiceCallback, IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Get all switch connections controllerd by current controller.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISwitchConnection> GetConnections();

        /// <summary>
        /// Get all units controlled by current controller
        /// </summary>
        /// <returns></returns>
        IEnumerable<UnitPhoneViewModel> GetUnits();

        /// <summary>
        /// Handler channel click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChannelClicked(object sender, PhoneEventArgs<HostPhoneViewModel> e);

        /// <summary>
        /// Handler unit click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UnitClicked(object sender, PhoneEventArgs<UnitPhoneViewModel> e);
        
        DateTime? FinalisedDate { get; }

        /// <summary>
        /// Call when the controller is being destroyed
        /// </summary>
        void Finalise();
        
        bool CanFinalise();

        /// <summary>
        /// Raise after controller finished its filalised process.
        /// </summary>
        event EventHandler OnFinalised;
    }
}
