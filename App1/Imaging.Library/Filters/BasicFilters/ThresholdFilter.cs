﻿namespace Imaging.Library.Filters.BasicFilters
{
    public class ThresholdFilter : FilterBase
    {
        private readonly byte _thresholdR;
        private readonly byte _thresholdG;
        private readonly byte _thresholdB;

        public ThresholdFilter(byte r, byte g, byte b)
        {
            _thresholdR = r;
            _thresholdG = g;
            _thresholdB = b;
        }

        public override void OnProcess()
        {
            for (var y = 0; y < Source.Height; y++)
            {
                for (var x = 0; x < Source.Width; x++)
                {
                    var pixel = Source.Map[y][x];

                    if (pixel.R < _thresholdR)
                        pixel.R = 0;

                    if (pixel.G < _thresholdG)
                        pixel.G = 0;

                    if (pixel.B < _thresholdB)
                        pixel.B = 0;

                    Source.Map[y][x] = pixel;
                }
            }
        }
    }
}