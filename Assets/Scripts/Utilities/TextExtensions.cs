/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

public static class TextExtension {

    public static void ReduceUntilFitsVertically(this Text text, int fontSize) {
        int minimum = 1;
        text.fontSize = fontSize;
        while (text.IsOverflowingVertical() && fontSize > minimum) {
            fontSize -= 1;
            text.fontSize = fontSize;
        }
    }

    public static void ReduceUntilFitsHorizontally(this Text text, int fontSize) {
        int minimum = 1;
        text.fontSize = fontSize;
        while (text.IsOverflowingHorizontal() && fontSize > minimum) {
            fontSize -= 1;
            text.fontSize = fontSize;
        }
    }

    public static bool IsOverflowing(this Text text) {
        return text.IsOverflowingVertical() || text.IsOverflowingHorizontal();
    }

    public static bool IsOverflowingVertical(this Text text) {
        RectTransform rt = text.GetComponent<RectTransform>();
        float preferredHeight = LayoutUtility.GetPreferredHeight(rt);
        return preferredHeight > rt.rect.height;
    }

    public static bool IsOverflowingHorizontal(this Text text) {
        RectTransform rt = text.GetComponent<RectTransform>();
        float preferredWidth = LayoutUtility.GetPreferredWidth(rt);
        return preferredWidth > rt.rect.width;
    }

}
