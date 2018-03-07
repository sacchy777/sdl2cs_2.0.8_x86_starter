/*
  Copyright (C) 2017 sada.gussy <sada.gussy@gmail.com>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely.
 */

/* SDL_ttf truetype font rendering test in c sharp */

using SDL2;
using System.Runtime.InteropServices; // Marshal

class TestSpriteMinimal {
    static readonly int WindowWidth = 1280;
    static readonly int WindowHeight = 720;
    static readonly int NumSprites = 100;
    static readonly int MaxSpeed = 1;

    static System.IntPtr Sprite;
    static SDL.SDL_Rect [] Positions = new SDL.SDL_Rect[NumSprites];
    static SDL.SDL_Rect [] Velocities = new SDL.SDL_Rect[NumSprites];
    static int Sprite_w = 0;
    static int Sprite_h = 0;

    static System.IntPtr Renderer;
    static int Done = 0;
    
    static System.IntPtr Font;
    static int FontSize = 36;
    static int NumFrame = 0;


    static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    static readonly double Fps = 60.0;
    static readonly double frameInterval = 1000.0 / Fps;
    static double lastTicks = 0;
    static bool Skip = false;
    static int FrameCount = 0;
    
    static void Quit(int rc) {
    }
    
    static void DrawText(string text, int x, int y, SDL.SDL_Color color, System.IntPtr renderer) {
	var src = new SDL.SDL_Rect();
	var dest = new SDL.SDL_Rect();
	//System.IntPtr temp = SDL_ttf.TTF_RenderUTF8_Blended(Font, text, TextColor);
	System.IntPtr temp = SDL_ttf.TTF_RenderUTF8_Solid(Font, text, color);
	SDL.SDL_Surface sur = (SDL.SDL_Surface) Marshal.PtrToStructure(temp, typeof(SDL.SDL_Surface));
	src.x = 0; src.y = 0; src.w = sur.w; src.h = sur.h;
	dest.x = x; dest.y = y; dest.w = sur.w; dest.h = sur.h;
	var tex = SDL.SDL_CreateTextureFromSurface(renderer, temp);
	SDL.SDL_FreeSurface(temp);
	SDL.SDL_RenderCopy(renderer, tex, ref src, ref dest);
	SDL.SDL_DestroyTexture(tex);
    }
    
    static int LoadSprite(string file, System.IntPtr renderer) {
	
	System.IntPtr temp = SDL.SDL_LoadBMP(file);
	    
	if (temp == null) {
	    return -1;
	}
	
	SDL.SDL_Surface sur = (SDL.SDL_Surface) Marshal.PtrToStructure(temp, typeof(SDL.SDL_Surface));
	Sprite_w = sur.w;
	Sprite_h = sur.h;
    
	Sprite = SDL.SDL_CreateTextureFromSurface(renderer, temp);
	
	if (Sprite == null) {
	}

	SDL.SDL_FreeSurface(temp);

	return 0;
    }
    
    static void MoveSprites(System.IntPtr renderer, System.IntPtr sprite) {

	SDL.SDL_Rect rect = new SDL.SDL_Rect();
	rect.x = 0; rect.y = 0; rect.w = Sprite_w; rect.h = Sprite_h;
	
	if (!Skip) {
	    SDL.SDL_SetRenderDrawColor(renderer, 0xA0, 0xA0, 0xA0, 0xFF);
	    SDL.SDL_RenderClear(renderer);
	}

	for (int i = 0; i < NumSprites; ++i) {
	    Positions[i].x += Velocities[i].x;
	    if ((Positions[i].x < 0) || (Positions[i].x >= (WindowWidth - Sprite_w))) {
		Velocities[i].x = -Velocities[i].x;
		Positions[i].x += Velocities[i].x;
	    }
	    Positions[i].y += Velocities[i].y;
	    if ((Positions[i].y < 0) || (Positions[i].y >= (WindowHeight - Sprite_h))) {
		Velocities[i].y = -Velocities[i].y;
		Positions[i].y += Velocities[i].y;
	    }
	    if (!Skip) {
		SDL.SDL_RenderCopy(renderer, sprite, ref rect, ref Positions[i]);
	    }
	}
	SDL.SDL_Color color = new SDL.SDL_Color{r=0x00, g=0x00, b=0xff, a=0x00};
	int x = 0;
	int y = 0;
	DrawText(System.String.Format("こんにちわ世界 \n frame {0}", FrameCount), x, y, color, renderer);
	DrawText(System.String.Format("Memory {0}", System.GC.GetTotalMemory(false)), 0, 120, color, renderer);
	NumFrame ++;
	if (!Skip) {
	    SDL.SDL_RenderPresent(renderer);
	}
    }

    static void TimerUpdate() {
	FrameCount ++;
	double currentTicks = (double)sw.ElapsedMilliseconds;
	double sleepTicks = frameInterval - (currentTicks - lastTicks);
	lastTicks += frameInterval;
	if(sleepTicks > 0) {
	    System.Threading.Thread.Sleep((int)sleepTicks);
	    Skip = false;
	}else {
	    Skip = true;
	}
    }
    static void Loop() {
    	SDL.SDL_Event ev;
 	while (SDL.SDL_PollEvent(out ev) != 0) {
	    if (ev.type == SDL.SDL_EventType.SDL_QUIT || ev.type == SDL.SDL_EventType.SDL_KEYDOWN) {
	    	Done = 1;
	    }
	}
	TimerUpdate();
	MoveSprites(Renderer, Sprite);
    }

    static public void Main() {

	System.IntPtr window;
	System.Random rand = new System.Random();
	
	//	SDL.SDL_LogSetPriority(SDL.SDL_LOG_CATEGORY_APPLICATION, SDL.SDL_LOG_PRIORITY_INFO);
	//	SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

	SDL.SDL_WindowFlags flags = 0;
//	flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
	
	if(SDL.SDL_CreateWindowAndRenderer(WindowWidth, WindowHeight, flags, out window, out Renderer) < 0){
	    Quit(2);
	}

	SDL.SDL_SetWindowTitle(window, "TestTtf");
	SDL.SDL_ShowCursor(0);
	
	SDL_ttf.TTF_Init();
	Font = SDL_ttf.TTF_OpenFont("misaki_gothic.ttf", FontSize);

	if(LoadSprite("icon.bmp", Renderer) < 0) {
	    Quit(2);
	}

	for (int i = 0; i < NumSprites; ++i) {
	    Positions[i].x = rand.Next(WindowWidth - Sprite_w);
	    Positions[i].y = rand.Next(WindowHeight - Sprite_h);
	    Positions[i].w = Sprite_w;
	    Positions[i].h = Sprite_h;
	    Velocities[i].x = 0;
	    Velocities[i].y = 0;
	    while (Velocities[i].x == 0 && Velocities[i].y == 0) {
		Velocities[i].x = rand.Next(MaxSpeed * 2 + 1) - MaxSpeed;
		Velocities[i].y = rand.Next(MaxSpeed * 2 + 1) - MaxSpeed;
	    }
	    
	}

	Done = 0;

	sw.Start();
	while (Done == 0) {
	    Loop();
	}
	
	SDL.SDL_DestroyRenderer(Renderer);
	SDL.SDL_DestroyWindow(window);
	SDL.SDL_Quit();

	SDL_ttf.TTF_CloseFont(Font);
	SDL_ttf.TTF_Quit();

	
	Quit(0);
	
    }
    
}
