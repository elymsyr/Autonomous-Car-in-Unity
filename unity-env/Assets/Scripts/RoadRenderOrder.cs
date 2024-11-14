using UnityEngine;

public class SetRenderQueueByHierarchy : MonoBehaviour
{
    public int baseRenderQueue = 3000;
    public int depthRecursive = 0;

    void Start()
    {
        // Start the recursive process from the root GameObject
        SetRenderQueueRecursively(transform, baseRenderQueue, depthRecursive - 1);
    }

    // Recursive function to traverse all child objects and assign render queues
    void SetRenderQueueRecursively(Transform parent, int currentRenderQueue, int depth)
    {
        int count = 0;
        // Get all child objects of the current parent
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // Calculate the render queue for this child based on the depth and index in the hierarchy
            int childRenderQueue = currentRenderQueue + (depth * 100) + i;

            // Assign material and set the render queue for the child's Renderer
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                // Access the existing material of the sub-child
                Material existingMaterial = childRenderer.material;

                // Set the render queue on the existing material
                existingMaterial.renderQueue = childRenderQueue;

                // Apply the updated material back to the Renderer (optional if auto-applied)
                childRenderer.material = existingMaterial;

                // Debug log for visual confirmation (optional)
                count += 1;
            }

            // Recursively call this function for the child's children
            if (depth - 1 >= 0){SetRenderQueueRecursively(child, currentRenderQueue, depth - 1);}
        }
        Debug.Log($"Set render queue {count} for {parent.name}");
    }
}