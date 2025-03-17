using Cysharp.Threading.Tasks;
using GameSdk.UnityContainer.VContainer;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Project.VContainers
{
    public class ProjectScope : VContainerScope
    {
        [SerializeField] private UIDocument _document;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_document).As<UIDocument>();
            builder.Register<DataSourceResolver>(Lifetime.Singleton).As<IDataSourceResolver>();
            builder.RegisterBuildCallback(container => { SetDataSource(container.Resolve<IDataSourceResolver>()).Forget(); });
        }

        private async UniTaskVoid SetDataSource(IDataSourceResolver dataSourceResolver)
        {
            await UniTask.WaitUntil(() => _document.runtimePanel != null);

            _document.runtimePanel.visualTree.dataSource = dataSourceResolver;
        }
    }
}
