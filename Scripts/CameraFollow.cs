using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   [SerializeField] public Transform player; // Referência ao transform do jogador
   [SerializeField] public float smoothSpeed = 5f; // Velocidade de suavização da câmera
   [SerializeField] public float minX, maxX; // Limites para movimentação no eixo X

    private void LateUpdate()
    {
        // Verificar se o player ainda existe
        if (player != null)
        {
            Vector3 targetPosition = player.position; // Posição alvo é a posição do jogador
            targetPosition.z = -5f; // Fixa a profundidade da câmera
            targetPosition.y = 0.45f; // Fixa o eixo Y da câmera

            // Suavização com precisão baseada no tempo
            Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // Clamp apenas no eixo X
            smoothPosition.x = Mathf.Clamp(smoothPosition.x, minX, maxX); // Restringe a posição X entre minX e maxX

            // Arredondamento de posição para evitar flicker gráfico
            smoothPosition.x = Mathf.Round(smoothPosition.x * 100f) / 100f; // Arredonda X para duas casas decimais
            smoothPosition.y = Mathf.Round(smoothPosition.y * 100f) / 100f; // Arredonda Y para duas casas decimais

            transform.position = smoothPosition; // Atualiza a posição da câmera
        }
    }
}