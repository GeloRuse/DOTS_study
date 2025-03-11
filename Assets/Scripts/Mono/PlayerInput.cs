using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private EntityManager entityManager;
    private EntityQuery entityQuery;
    private Entity inputEntity;
    private MouseInput mouseInput;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<MouseInput>().Build(entityManager);
        inputEntity = entityQuery.GetSingletonEntity();
    }

    // Update is called once per frame
    private void Update()
    {
        mouseInput.mousePosition = MouseWorldPosition.Instance.GetPosition();

        if (MouseWorldPosition.Instance.GetClick())
        {
            mouseInput.mouseClick = true;
        }
        else
        {
            mouseInput.mouseClick = false;
        }
        entityManager.SetComponentData(inputEntity, mouseInput);
    }
}
