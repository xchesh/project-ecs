using GameSdk.Services.RemoteConfig;

namespace Project.Services.RemoteConfig
{
    public class ProjectRemoteConfigAttribution : IRemoteConfigAttribution
    {
        public (IUserAttributes user, IAppAttributes app, IFilterAttributes filter) GetAttributes()
        {
            return (GetUserAttributes(), GetAppAttributes(), GetFilterAttributes());
        }

        public IUserAttributes GetUserAttributes()
        {
            return new UserAttributes();
        }

        public IAppAttributes GetAppAttributes()
        {
            return new AppAttributes();
        }

        public IFilterAttributes GetFilterAttributes()
        {
            return new FilterAttributes();
        }
    }
}
