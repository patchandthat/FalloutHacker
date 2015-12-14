using System;
using System.Collections.Generic;
using Tesseract;

namespace FalloutHacker
{
    public class RegionOfInterest
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }
    }

    public class TerminalReader : ITerminalReader
    {
        public TerminalData AnalyseImage(string imagePath)
        {
            var raw = new List<string>();

            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            using (var image = Pix.LoadFromFile(imagePath))
            using (Page page = engine.Process(image, PageSegMode.SingleBlock)) //Might want single block mode
            {
                string text = page.GetText();
                raw.Add(text);
                var confidence = page.GetMeanConfidence();
                raw.Add($"Confidence: {confidence}");
                using (var iterator = page.GetIterator())
                {
                    //Iterate the region of interest
                    //Probably want to drag a bounding box for the RoI in GUI.
                }
            }

            return new TerminalData(raw, raw);
        }

        public TerminalData AnalyseImage(string imagePath, RegionOfInterest roi)
        {
            var raw = new List<string>();
            var rect = Rect.FromCoords(roi.X1, roi.Y1, roi.X2, roi.Y2);

            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            using (var image = Pix.LoadFromFile(imagePath))
            using (Page page = engine.Process(image, rect, PageSegMode.Auto)) //Might want single block mode
            {
                string text = page.GetText();
                raw.Add(text);
                var confidence = page.GetMeanConfidence();
                raw.Add($"Confidence: {confidence}");
                using (var iterator = page.GetIterator())
                {
                    //Iterate the region of interest
                    //Probably want to drag a bounding box for the RoI in GUI.
                }
            }

            return new TerminalData(raw, raw);
        }
    }
}
