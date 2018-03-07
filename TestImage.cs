/*
  Copyright (C) 2017 sada.gussy <sada.gussy@gmail.com>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely.
 */

/* SDL_Image png loading test in c sharp */

using SDL2;

class TestSpriteMinimal {
    static readonly int WindowWidth = 640;
    static readonly int WindowHeight = 480;
    static readonly int NumSprites = 100;
    static readonly int MaxSpeed = 1;

    static System.IntPtr Sprite;
    static SDL.SDL_Rect [] Positions = new SDL.SDL_Rect[NumSprites];
    static SDL.SDL_Rect [] Velocities = new SDL.SDL_Rect[NumSprites];
    static int Sprite_w = 0;
    static int Sprite_h = 0;

    static System.IntPtr Renderer;
    static int Done = 0;

    static void Quit(int rc) {
    }

    static int LoadSprite(string file, System.IntPtr renderer) {
	Sprite = SDL_image.IMG_LoadTexture(renderer, file);
	uint dummy1;
	int dummy2;
	SDL.SDL_QueryTexture(Sprite, out dummy1, out dummy2, out Sprite_w, out Sprite_h);
	if(Sprite_w == 0) {
	    //	    SDL.SDL_LogError(SDL.SDL_LOG_CATEGORY_APPLICATION, "{0}:{1}\n", __arglist(file, SDL.SDL_GetError()));
	    System.Console.WriteLine(System.String.Format("{0}:{1}", file, SDL.SDL_GetError()));
	    return -1;
	}
	return 0;
    }
    
    static void MoveSprites(System.IntPtr renderer, System.IntPtr sprite) {

	SDL.SDL_Rect rect = new SDL.SDL_Rect();
	rect.x = 0; rect.y = 0; rect.w = Sprite_w; rect.h = Sprite_h;

	SDL.SDL_SetRenderDrawColor(renderer, 0xA0, 0xA0, 0xA0, 0xFF);
	SDL.SDL_RenderClear(renderer);

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

	    SDL.SDL_RenderCopy(renderer, sprite, ref rect, ref Positions[i]);
	}
	SDL.SDL_RenderPresent(renderer);
    }

    static void Loop() {
	
    	SDL.SDL_Event ev;
	
 	while (SDL.SDL_PollEvent(out ev) != 0) {
	    if (ev.type == SDL.SDL_EventType.SDL_QUIT || ev.type == SDL.SDL_EventType.SDL_KEYDOWN) {
	    	Done = 1;
	    }
	}
	MoveSprites(Renderer, Sprite);
    }

    static public void Main() {

	System.IntPtr window;
	System.Random rand = new System.Random();
	
	//SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

	SDL.SDL_WindowFlags flags = 0;
	//	flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
	
	if(SDL.SDL_CreateWindowAndRenderer(WindowWidth, WindowHeight, flags, out window, out Renderer) < 0){
	    Quit(2);
	}

	SDL.SDL_SetWindowTitle(window, "TestSpriteMinimal");


	if(LoadSprite("icon.png", Renderer) < 0) {
	    Quit(2);
	    return;
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

	while (Done == 0) {
	    Loop();
	}
	
	SDL.SDL_DestroyRenderer(Renderer);
	SDL.SDL_DestroyWindow(window);
	SDL.SDL_Quit();

	Quit(0);
	
    }
    
}
