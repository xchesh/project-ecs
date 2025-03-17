using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
public static class UIExtensions
{
    private static  Vector2 _vector2Zero = new (0f, 0f);

     /// <summary>
     /// This method adjust the letter size according to the text area.
     /// </summary>
     /// <param name="isAuto">If using manual control this must set "false".</param>
     /// <param name="isTextLengthIncrease">Need to this value for auto control. We must to know letter is added or deleted?</param>
    public static void FixTextSize(this TextElement text, bool isAuto, bool isTextLengthIncrease = true)
    {
        if(text.contentRect.size == _vector2Zero)
        {
            Debug.Log($"FixTextSize-ContentRect size is zero({text.contentRect.size})!!!");
            return;
        }

        if(text.resolvedStyle.fontSize <= 1)
        {
            Debug.Log($"FixTextSize-Font size({text.resolvedStyle.fontSize}) is less than one!!!");
            return;
        }

        string afterTrim = text.text.Trim();
        if(string.IsNullOrEmpty(afterTrim))
        {
            Debug.Log("FixTextSize-Font is empty!");
            return;
        }

        float labelContentSizeX = text.contentRect.size.x;
        float labelContentSizeY = text.contentRect.size.y;

        Vector2 measureTextSize = CalculateMeasureTextSize(text, labelContentSizeX, labelContentSizeY);

        //Decrease the font size
        while((!isAuto || isTextLengthIncrease) && measureTextSize.x > labelContentSizeX)
        {
            text.style.fontSize = text.resolvedStyle.fontSize - 1;
            measureTextSize = CalculateMeasureTextSize(text, labelContentSizeX, labelContentSizeY);
        }

        //Increase the font size
        while((!isAuto || !isTextLengthIncrease) && measureTextSize.x < labelContentSizeX && measureTextSize.y <labelContentSizeY)
        {
            text.style.fontSize = text.resolvedStyle.fontSize + 1;
            measureTextSize = CalculateMeasureTextSize(text, labelContentSizeX, labelContentSizeY);

            // We need this control because last visual of text can be oversize of the text area.
            if(measureTextSize.x > labelContentSizeX)
            {
                text.style.fontSize = text.resolvedStyle.fontSize - 1;
                Debug.Log("FixLabelTextSize-measureTextSize-increase-last check!");
            }
        }
    }

    private static Vector2 CalculateMeasureTextSize(TextElement label, float labelContentSizeX, float labelContentSizeY)
    {
        return label.MeasureTextSize(label.text, labelContentSizeX, VisualElement.MeasureMode.Undefined, labelContentSizeY, VisualElement.MeasureMode.Undefined);
    }
}
