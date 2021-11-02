using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid {

	private int[,] data;
	private int width = 5;
	private int height = 5;

	public TileGrid (int w, int h) {
		width = w;
		height = h;

		data = new int[width,height];

		for (int iy = 0; iy < height; iy++) {
			for (int ix = 0; ix < width; ix++) {
				data [ix, iy] = 0;
			}
		}
	}

	public string DataAsString() {
		string str = "";

		string[] values = {
			".",
			"X",
			"<color=#ff0000>O</color>",
			"<color=#00ff00>O</color>"
		};

		for (int iy = 0; iy < height; iy++) {
			for (int ix = 0; ix < width; ix++) {
				str += values[data [ix, iy]];
			}

			if (iy < height - 1) {
				str += "\n";
			}
		}

		return str;
	}

	public void AddNumber(int num, int x, int y) {
		data [x, y] = num;
	}

	public void Slide(int x, int y) {
		int safeCheck = 0;
		while (SlideOnce (x, y) && safeCheck < 999) {
			safeCheck++;
		}
	}

	public bool SlideOnce(int x, int y) {
		bool didSlide = false;

		for (int iy = 0; iy < height; iy++) {
			for (int ix = 0; ix < width; ix++) {

				if (InBounds(ix + x, iy + y) && TileFree(ix + x, iy + y) && CanSlide(ix, iy)) {
					data [ix + x, iy + y] = data [ix, iy];
					data [ix, iy] = 0;
					didSlide = true;
				}

			}
		}

		return didSlide;
	}

	public bool TileFree(int x, int y) {
		return (data [x, y] == 0);
	}

	public bool InBounds(int x, int y) {
		return (x >= 0 && x < width && y >= 0 && y < height);
	}

	public bool CanSlide(int x, int y) {
		return (data [x, y] > 1);
	}
}
