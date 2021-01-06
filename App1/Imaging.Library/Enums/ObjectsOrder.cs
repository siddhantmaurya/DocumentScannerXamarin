﻿namespace Imaging.Library.Enums
{
    public enum ObjectsOrder
    {
        /// <summary>
        /// Unsorted order (as it is collected by algorithm).
        /// </summary>
        None,

        /// <summary>
        /// Objects are sorted by size in descending order (bigger objects go first).
        /// Size is calculated as <b>Width * Height</b>.
        /// </summary>
        Size,

        /// <summary>
        /// Objects are sorted by area in descending order (bigger objects go first).
        /// </summary>
        Area,

        /// <summary>
        /// Objects are sorted by Y coordinate, then by X coordinate in ascending order
        /// (smaller coordinates go first).
        /// </summary>
        YX,

        /// <summary>
        /// Objects are sorted by X coordinate, then by Y coordinate in ascending order
        /// (smaller coordinates go first).
        /// </summary>
        XY
    }
}