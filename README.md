# Unity Sprite GPU Instancing
A Unity SpriteRenderer GPU Instancing Implementation Demo

![Batching SpriteRenderer](https://github.com/ownself/UnitySpriteGPUInstancing/blob/main/result.png)

Unity Version : 2019.4

### Key features:
* Replace SpriteRenderer with custom MeshRenderer at runtime
* Using Vertext shader to represent PixelsPerUnit and offset in Sprite
* Combine textures with Texture Array, so textures need to be at same format and size
* Use MaterialPropertyBlock for GPU Instancing feature of Unity

### Related Blog:
For more information, please check out my Chinese blog post: [Unity Sprite GPU Instancing](http://www.ownself.org/blog/2022/unity-sprite-gpu-instancing.html)
