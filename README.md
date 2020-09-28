# Scriptable-Object-Powered-Object-Pooling
A simple and lightweight Object Pooling solution for Unity Engine powerd by Scriptable Objects.

### HOW TO USE
1. Install the .unitypackage or clone/download the repo into your assets

1. Add the 'Object Pooler' script to a GameObject of your choice in your scene

1. Create a 'Poolable Object' asset by either right clicking in your project or selecting Assets > Create > Poolable Object
    - Give the Poolable Object a nice descriptive name, this gets used in any error messages
    - Provide it with the prefab you wish to associate with this poolable asset
    - Provide the number of how many should the pooler initially instansiate on Start()
    
1. Put your newly created poolable object asset into the 'Poolable Objects' array in the Object Pooler component you have in your scene

1. Repeat steps 3 and 4 for each prefab you want pooled.

1. When you want to spawn a pooled prefab, simply call ObjectPooler.SpawnObject and pass in a reference to the poolable object asset.

1. If you would like to "Destroy" an object, call ObjectPooler.ReturnToPool and pass in the object you wish to be deactivated as well as the poolable asset.

1. Enjoy your minimal garbage object pooling system!
