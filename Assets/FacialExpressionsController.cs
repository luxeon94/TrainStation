using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialExpressionsController : MonoBehaviour
{
    public TextAsset FacialExpressionsJSON;
    public FacialExpressions FE = new FacialExpressions();
    // Start is called before the first frame update
    void Start()
    {
        FE = JsonUtility.FromJson<FacialExpressions>(FacialExpressionsJSON.text);

        foreach (FacialExpression exp in FE.expressions)
        {
            Debug.Log("New expression: " + exp.id);
            foreach (FEWeight weights in exp.weights)
            {
                Debug.Log("    Blendshape n" + weights.blendShapeId + ": value=" + weights.blendShapeValue);
            }
        }
        Debug.Log("After foreach");

        //Give facial expression to char MaleNew1
        GameObject char1 = GameObject.Find("MaleNew1");
        CharacterExpressionsController expChar1 = char1.GetComponent<CharacterExpressionsController>();
        expChar1.activateFacialExpression(FE.expressions[0], 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
