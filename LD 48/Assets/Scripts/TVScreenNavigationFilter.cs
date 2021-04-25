using Listonos.NavigationSystem;

public class TVScreenNavigationFilter : NavigationFilter<int>
{
  new void Start()
  {
    base.Start();
    for (int i = 0; i < base.ActiveOnScreens.Length; i++)
    {
      GameManager.Instance.AddValidPage(base.ActiveOnScreens[i]);
    }
  }
}
