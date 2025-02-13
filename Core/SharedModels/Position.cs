﻿using SC2APIProtocol;

namespace Core.Model {
    public class Position : IPosition {
        public Point2D Point { get; set; }
        public Position(float x, float y) {
            Point = new Point2D { X = x, Y = y };
        }
    }
}
