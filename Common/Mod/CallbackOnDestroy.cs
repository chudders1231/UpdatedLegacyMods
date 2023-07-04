using System;
using UnityEngine;

namespace Common.Mod
{
    class CallbackOnDestroy : MonoBehaviour
    {
		public Action<GameObject> onDestroy = delegate { };

		private void OnDestroy()
		{
			onDestroy(gameObject);
		}
    }
}
