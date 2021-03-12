namespace NovaAlert.Bll.Controller
{
    /// <summary>
    /// Controller cho chế độ báo hiệu
    /// </summary>
    public class PrivateAlarmController: MainController
    {
        public PrivateAlarmController(ClientAppViewModel app):base(app, true)
        {
        }

        public override void OnDialCompleted(Entities.ChannelEventArgs e)
        {            
        }
    }
}
