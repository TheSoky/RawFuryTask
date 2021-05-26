using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	private class PoolInstance
	{
		//List containing all of objects in pool
		private List<GameObject> _pooledObjects = new List<GameObject>();

		//Key object which is pooled, reference for creeating new ones
		private GameObject _objectToPool;

		//Transform which serve as a parent for created objects
		private Transform _parentTransform;

		/// <summary>
		/// Constructor for Pool instance, creates pool of selected object in selected amount
		/// </summary>
		/// <param name="objectToPool">Prefab of the object to pool</param>
		/// <param name="amountToPool">How many instances should be created in time of initial pooling</param>
		/// <param name="parentTransform">Transform of a parent under which objects will be in hierarchy</param>
		public PoolInstance(GameObject objectToPool, int amountToPool, Transform parentTransform)
		{
			_parentTransform = parentTransform;
			_objectToPool = objectToPool;
			for(int i = 0; i < amountToPool; i++)
			{
				GameObject instantiatedObject = Instantiate(_objectToPool, parentTransform);
				instantiatedObject.SetActive(false);
				_pooledObjects.Add(instantiatedObject);
			}
		}


		/// <summary>
		/// Finds the first inactive object and pool and returns it. If none is found, creates new one, returns it and adds to pool.
		/// </summary>
		/// <returns>First available inactive GameObject</returns>
		public GameObject GetNextAvailableObject()
		{
			for(int i = 0; i < _pooledObjects.Count; i++)
			{
				if(!_pooledObjects[i].activeInHierarchy)
				{
					return _pooledObjects[i];
				}
			}
			//Available object not found, create a new one
			GameObject newCreatedObject = Instantiate(_objectToPool, _parentTransform);
			newCreatedObject.SetActive(false);
			_pooledObjects.Add(newCreatedObject);
			return newCreatedObject;
		}

	}

	private Dictionary<GameObject, PoolInstance> _pools = new Dictionary<GameObject, PoolInstance>();

	private static PoolManager instance = null;
	public static PoolManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake() 
	{
		if(instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			instance = this;
		}
	}

	/// <summary>
	/// Creates a pool for selected GameObject in selected amount, under selected parent
	/// </summary>
	/// <param name="objectToPool">Prefab of an object to pool</param>
	/// <param name="amountToPool">How many instances should be created</param>
	/// <param name="parentTransform">Parent under which the instances will be</param>
	public void AddNewPool(GameObject objectToPool, int amountToPool, Transform parentTransform)
	{
		PoolInstance newPool = new PoolInstance(objectToPool, amountToPool, parentTransform);
		_pools.Add(objectToPool, newPool);
	}

	/// <summary>
	/// Creates a pool for selected GameObject in selected amount, under selected parent
	/// </summary>
	/// <param name="objectToPool">Preefab of an object to pool</param>
	/// <param name="amountToPool">How many instances should be creeated</param>
	public void AddNewPool(GameObject objectToPool, int amountToPool)
	{
		PoolInstance newPool = new PoolInstance(objectToPool, amountToPool, transform);
		_pools.Add(objectToPool, newPool);
	}

	/// <summary>
	/// Creates multiple pools of GameObjects in selected amount for each, under selected parent
	/// </summary>
	/// <param name="newPools">Dictionary containing GameObject as key and number of instances as value</param>
	/// <param name="parentTransform">Parent transform for pooled GameObjects</param>
	public void AddNewPool(Dictionary<GameObject, int> newPools, Transform parentTransform)
	{
		foreach(KeyValuePair<GameObject, int> newPool in newPools)
		{
			PoolInstance pool = new PoolInstance(newPool.Key, newPool.Value, parentTransform);
			_pools.Add(newPool.Key, pool);
		}
	}

	/// <summary>
	/// Creates multiple pools of GameObjects in selected amount for each
	/// </summary>
	/// <param name="newPools">Dictionary containing GameObject as key and number of instances as value</param>
	public void AddNewPool(Dictionary<GameObject, int> newPools)
	{
		foreach(KeyValuePair<GameObject, int> newPool in newPools)
		{
			PoolInstance pool = new PoolInstance(newPool.Key, newPool.Value, transform);
			_pools.Add(newPool.Key, pool);
		}
	}


	/// <summary>
	/// Finds the first inactive object and pool and returns it. If none is found, creates new one, returns it and adds to pool
	/// </summary>
	/// <param name="objectToGet"></param>
	/// <returns>First available inactive GameObject</returns>
	public GameObject GetNextAvailableObject(GameObject objectToGet)
	{
		if(_pools.ContainsKey(objectToGet))
		{
			return _pools[objectToGet].GetNextAvailableObject();
		}
		else
		{
			Debug.LogError("Pool for " + objectToGet.name + " not found. null fetched");
			return null;
		}
	}
}
