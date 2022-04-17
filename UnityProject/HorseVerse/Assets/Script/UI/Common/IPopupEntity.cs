using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public interface IPopupEntity<T> where T : new()
{
    UniTask In();
    UniTask Out();
}