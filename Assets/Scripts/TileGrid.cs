﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGrid<T> {

	private T[,] data;
	private int width = 5;
	private int height = 5;

	public TileGrid (int w, int h) {
		width = w;
		height = h;

		data = new T[width,height];

		for (var iy = 0; iy < height; iy++) {
			for (var ix = 0; ix < width; ix++) {
				data[ix, iy] = default;
			}
		}
	}

	public void Set(T value, int x, int y) {
		data[x, y] = value;
	}

	public IEnumerable<T> GetNeighbours(int x, int y)
	{
		var list = new List<T>();
		AddIfNotDefault(x + 1, y, list);
		AddIfNotDefault(x - 1, y, list);
		AddIfNotDefault(x, y + 1, list);
		AddIfNotDefault(x, y - 1, list);
		// Debug.Log($"Finding neighbours for ({x},{y}) => {list.Count}");
		return list;
	}

	private void AddIfNotDefault(int x, int y, ICollection<T> list)
	{
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			// Debug.Log("Adding " + x + ", " + y);
			list.Add(data[x, y]);
		}
	}

	public IEnumerable<T> All()
	{
		return data.Cast<T>();
	}
}