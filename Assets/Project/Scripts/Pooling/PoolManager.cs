// using System;
// using System.Collections;
// using TetrisBlast.Project.Scripts.Animations;
// using TetrisBlast.Project.Scripts.Blocks;
// using TetrisBlast.Project.Scripts.Boards;
// using TetrisBlast.Project.Scripts.Core;
// using TetrisBlast.Project.Scripts.Grid;
// using TetrisBlast.Project.Scripts.Shapes;
// using TetrisBlast.Project.Scripts.Utils;
// using TetrisBlast.Project.Scripts.Utils.Helpers;
// using UnityEngine;
//
// namespace TetrisBlast.Project.Scripts.Pooling
// {
//     /// <summary>
//     /// Singleton manager for all object pools in TetrisBlast
//     /// </summary>
//     public sealed class PoolManager : MonoSingleton<PoolManager>
//     {
//         [Header("TetrisBlast Pools")]
//         [SerializeField] private PoolStats<ColoredCube> coloredCubePoolStats;
//         [SerializeField] private PoolStats<Piece> blockPiecePoolStats;
//         [SerializeField] private PoolStats<ShapeCube> shapeCubePoolStats;
//         [SerializeField] private PoolStats<GhostPiece> ghostCubePoolStats;
//
//         [Header("Particle Pools")]
//         [SerializeField] private PoolStats<ColoredCubeParticle> cubeDestroyPoolStats;
//         [SerializeField] private PoolStats<ParticleSystem> iceBreakPoolStats;
//         
//         [Header("Glow Pools")]
//         [SerializeField] private PoolStats<SpriteRenderer> cellGlowPoolStats;
//         [SerializeField] private PoolStats<SpriteRenderer> doubleRowGlowPoolStats;
//         [SerializeField] private PoolStats<SpriteRenderer> tripleRowGlowPoolStats;
//
//         [Header("Puzzle Pools")]
//         [SerializeField] private PoolStats<IceBlock> iceBlockPoolStats;
//
//         private Pool<ColoredCube> _coloredCubePool;
//         private Pool<Piece> _blockPiecePool;
//         private Pool<ShapeCube> _shapeCubePool;
//         private Pool<GhostPiece> _ghostCubePool;
//         private Pool<ColoredCubeParticle> _cubeDestroyPool;
//         private Pool<ParticleSystem> _iceBreakPool;
//         private Pool<SpriteRenderer> _cellGlowPool;
//         private Pool<SpriteRenderer> _doubleRowGlowPool;
//         private Pool<SpriteRenderer> _tripleRowGlowPool;
//         private Pool<IceBlock> _iceBlockPool;
//
//         private void Awake()
//         {
//             if (coloredCubePoolStats.Prefab)
//                 _coloredCubePool = new(coloredCubePoolStats, transform);
//
//             if (blockPiecePoolStats.Prefab)
//                 _blockPiecePool = new(blockPiecePoolStats, transform);
//
//             if (shapeCubePoolStats.Prefab)
//                 _shapeCubePool = new(shapeCubePoolStats, transform);
//
//             if (ghostCubePoolStats.Prefab)
//                 _ghostCubePool = new(ghostCubePoolStats, transform);
//
//             if (cubeDestroyPoolStats.Prefab)
//                 _cubeDestroyPool = new(cubeDestroyPoolStats, transform);
//
//             if (iceBreakPoolStats.Prefab)
//                 _iceBreakPool = new(iceBreakPoolStats, transform);
//
//             if (cellGlowPoolStats.Prefab)
//                 _cellGlowPool = new(cellGlowPoolStats, transform);
//
//             if (doubleRowGlowPoolStats.Prefab)
//                 _doubleRowGlowPool = new(doubleRowGlowPoolStats, transform);
//
//             if (tripleRowGlowPoolStats.Prefab)
//                 _tripleRowGlowPool = new(tripleRowGlowPoolStats, transform);
//
//             if (iceBlockPoolStats.Prefab)
//                 _iceBlockPool = new(iceBlockPoolStats, transform);
//         }
//
//         public ColoredCube SpawnColoredCube(Vector3 position, GameColors color)
//         {
//             if (_coloredCubePool == null)
//                 return null;
//
//             ColoredCube cube = _coloredCubePool.Spawn(position);
//             cube.Initialize(color);
//             return cube;
//         }
//
//         public void ReturnColoredCube(ColoredCube cube)
//         {
//             if (_coloredCubePool == null || !cube) return;
//             _coloredCubePool.Return(cube);
//         }
//
//         public Piece SpawnBlockPiece(Vector3 position)
//             => _blockPiecePool != null
//                 ? _blockPiecePool.Spawn(position)
//                 : null;
//
//         public void ReturnBlockPiece(Piece piece)
//         {
//             if (_blockPiecePool == null || !piece) return;
//             _blockPiecePool.Return(piece);
//         }
//
//         public ShapeCube SpawnShapeCube(Vector3 position, Piece owner, GameColors color)
//         {
//             if (_shapeCubePool == null)
//                 return null;
//
//             ShapeCube cube = _shapeCubePool.Spawn();
//             cube.DisableTrail();
//             cube.transform.position = position;
//             cube.Initialize(owner, color);
//             return cube;
//         }
//
//         public void ReturnShapeCube(ShapeCube cube)
//         {
//             if (_shapeCubePool == null || !cube) return;
//
//             cube.DisableTrail();
//             _shapeCubePool.Return(cube);
//         }
//
//         public GhostPiece SpawnGhostCube(Vector3 position)
//         {
//             if (_ghostCubePool == null)
//                 return null;
//
//             GhostPiece piece = _ghostCubePool.Spawn();
//             piece.transform.position = position;
//             return piece;
//         }
//
//         public void ReturnGhostCube(GhostPiece piece)
//         {
//             if (_ghostCubePool == null || !piece) return;
//             _ghostCubePool.Return(piece);
//         }
//
//         public ColoredCubeParticle SpawnCubeDestroyEffect(Vector3 position, GameColors color)
//         {
//             if (_cubeDestroyPool == null)
//                 return null;
//             
//             Vector3 calculatedPosition = position;
//             calculatedPosition.y += 1f;
//
//             ColoredCubeParticle effect = _cubeDestroyPool.Spawn(calculatedPosition);
//             effect.transform.localScale = Vector3.one;
//             
//             effect.InitParticles(color);
//             effect.Play();
//
//             StartCoroutine(ReturnParticleAfterDuration(effect.Particle, () => _cubeDestroyPool.Return(effect)));
//
//             return effect;
//         }
//
//         public ParticleSystem SpawnIceBreakEffect(Vector3 position)
//         {
//             if (_iceBreakPool == null)
//                 return null;
//
//             ParticleSystem effect = _iceBreakPool.Spawn(position);
//             effect.Play();
//
//             StartCoroutine(ReturnParticleAfterDuration(effect, () => _iceBreakPool.Return(effect)));
//
//             return effect;
//         }
//
//         public SpriteRenderer SpawnCellGlowEffect(Vector3 position) 
//             => _cellGlowPool?.Spawn(position);
//
//         private void ReturnCellGlowEffect(SpriteRenderer effect)
//         {
//             if (_cellGlowPool == null || !effect) return;
//             _cellGlowPool.Return(effect);
//         }
//
//         public SpriteRenderer SpawnDoubleRowGlowEffect(Vector3 position)
//             => _doubleRowGlowPool?.Spawn(position);
//
//         private void ReturnDoubleRowGlowEffect(SpriteRenderer effect)
//         {
//             if (_doubleRowGlowPool == null || !effect) return;
//             _doubleRowGlowPool.Return(effect);
//         }
//
//         public SpriteRenderer SpawnTripleRowGlowEffect(Vector3 position)
//             => _tripleRowGlowPool?.Spawn(position);
//
//         private void ReturnTripleRowGlowEffect(SpriteRenderer effect)
//         {
//             if (_tripleRowGlowPool == null || !effect) return;
//             _tripleRowGlowPool.Return(effect);
//         }
//
//         public void ReturnGlowEffect(SpriteRenderer effect, GlowType type)
//         {
//             if (!effect) return;
//
//             switch (type)
//             {
//                 case GlowType.Single:
//                     ReturnCellGlowEffect(effect);
//                     break;
//                 case GlowType.Double:
//                     ReturnDoubleRowGlowEffect(effect);
//                     break;
//                 case GlowType.Triple:
//                     ReturnTripleRowGlowEffect(effect);
//                     break;
//             }
//         }
//
//         public IceBlock SpawnIceBlock(Vector3 position)
//         {
//             IceBlock iceBlock = _iceBlockPool?.Spawn(position);
//             return iceBlock;
//         }
//
//         public void ReturnIceBlock(IceBlock iceBlock)
//         {
//             if (_iceBlockPool == null || !iceBlock) return;
//             _iceBlockPool.Return(iceBlock);
//         }
//
//         private static IEnumerator ReturnParticleAfterDuration(ParticleSystem ps, Action onReturn)
//         {
//             float duration = ps.main.duration + ps.main.startLifetime.constantMax;
//             yield return WaitHelper.WaitForSeconds(duration);
//
//             onReturn?.Invoke();
//         }
//     }
// }
