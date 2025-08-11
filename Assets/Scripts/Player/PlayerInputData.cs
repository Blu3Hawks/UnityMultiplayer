using Fusion;
using UnityEngine;

namespace Player
{
    public struct PlayerInputData : INetworkInput
    {
        public Vector3 Movementvector;
    }
}