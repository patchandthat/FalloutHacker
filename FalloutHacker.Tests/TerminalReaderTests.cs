using System;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FalloutHacker.Tests
{
    public class TerminalReaderTests
    {
        internal const string Terminal1 = "Images\\2015-12-13_00001.jpg";
        internal const string Terminal2 = "Images\\2015-12-13_00003.jpg";
        internal const string LowResSample = "Images\\Standard2.jpg";
        internal const string TestImageFilePath3 = "Images\\phototest.jpg";
        internal const string PunctuationSample = "Images\\sample.jpg";

        internal const string RegionOfInterest = "Images\\roi.";
        internal static readonly string[] roiExts = new []{"bmp", "jpg", "png", "tif"};
        
        public class ReadingRawInput
        {
            private readonly ITerminalReader _sut = new TerminalReader();

            private readonly ITestOutputHelper _output;

            public ReadingRawInput(ITestOutputHelper output)
            {
                _output = output;
            }

            [Fact]
            public void IveNoIdeaHowThisLibraryWorksJustGetItToInitialise()
            {
                var result = _sut.AnalyseImage(Terminal2);
                Assert.NotNull(result);
            }

            [Fact]
            public void JustSeeWhatTextIsReturnedByDefaultAndFuckAboutwithSettingsToMakeItwork()
            {
                var result = _sut.AnalyseImage(LowResSample);
                var rawData = result.Lines.FirstOrDefault();

                const string expectedString =
@"Public Sub ProcessYourOwnBitmap()
OCR1.BitmapImage = myBitmap
OCR1.Process()
TextBox1.Text = OCR1.Text
End Sub";

                _output.WriteLine(rawData);
                Assert.Equal(expectedString, rawData);
            }

            [Fact]
            public void AnotherSlightlyHigherResSampleImage()
            {
                var result = _sut.AnalyseImage(TestImageFilePath3);
                var rawData = result.Lines.FirstOrDefault();
                const string expectedString =
"This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                _output.WriteLine(rawData);
                Assert.Equal(expectedString, rawData);
            }

            [Fact]
            public void AnotherSampleImageWithPunctuation()
            {
                var result = _sut.AnalyseImage(PunctuationSample);
                var rawData = result.Lines.FirstOrDefault();
                const string expectedString =
"The (quick) brown {fox} jumps! over the\n $3,456.78 <lazy> #90 dog & duck/goose, as\n 12.5% of E-mail from\n aspammer@website.com is spam?";

                _output.WriteLine(rawData);
                Assert.Equal(expectedString, rawData);
            }

            [Fact]
            public void PrintTerminal1()
            {
                DebugPrintTerminalImage(Terminal1);
            }

            [Fact]
            public void PrintTerminal1WithBoundingRegion()
            {
                var roi = new RegionOfInterest();
                roi.X1 = 550;
                roi.Y1 = 230;
                roi.X2 = 1255;
                roi.Y2 = 880;

                DebugPrintTerminalImage(Terminal1, roi);
            }

            [Fact]
            public void PrintTerminal2()
            {
                DebugPrintTerminalImage(Terminal2);
            }

            [Fact]
            public void ExhaustivelyTryRoiAllFileTypesAndAllEnumValues()
            {
                foreach (var extension in roiExts)
                {
                    DebugPrintAllTerminalImage($"{RegionOfInterest}{extension}");
                }
            }

            private void DebugPrintTerminalImage(string image, RegionOfInterest roi = null)
            {
                _output.WriteLine("");
                _output.WriteLine($"Image: {image}");
                _output.WriteLine("");
                TerminalData result;
                if (roi != null)
                {
                    result = _sut.AnalyseImage(image, roi);
                }
                else
                {
                    result = _sut.AnalyseImage(image);
                }

                var rawData = result.Lines.FirstOrDefault();
                _output.WriteLine(rawData);
            }

            private void DebugPrintAllTerminalImage(string image, RegionOfInterest roi = null)
            {
                TerminalData result;
                if (roi != null)
                {
                    result = _sut.AnalyseImage(image, roi);
                }
                else
                {
                    result = _sut.AnalyseImage(image);
                }

                foreach (string line in result.Lines)
                {
                    _output.WriteLine(line);
                }
            }

            //todo
            /*
                try some pre-processing of the image, break it down line by line without the memory address parts
            */
        }
    }
}
