/*
  Copyright (C) 2017 sada.gussy <sada.gussy@gmail.com>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely.
 */

/* SDL_Mixer ogg/vorbis, raw wave loading test in c sharp */

using SDL2;

class TestMixer {
    public static void Main() {
	int audio_rate = 44100;
	ushort audio_format = SDL.AUDIO_S16SYS;
	int audio_channels = 2;
	int audio_buffers = 4096;
	int audio_volume = SDL_mixer.MIX_MAX_VOLUME;
	int timeleft = 50;
	SDL.SDL_Init(SDL.SDL_INIT_AUDIO);
	if (SDL_mixer.Mix_OpenAudio(audio_rate, audio_format, audio_channels, audio_buffers) < 0) {
	    return;
	} else {
	    SDL_mixer.Mix_QuerySpec(out audio_rate, out audio_format, out audio_channels);
	}
	SDL_mixer.Mix_AllocateChannels(32);
	SDL_mixer.Mix_VolumeMusic(audio_volume/3);
	
	System.IntPtr music = SDL_mixer.Mix_LoadMUS("Tetris_theme.ogg");
	System.IntPtr wave = SDL_mixer.Mix_LoadWAV("Meow.ogg");
	SDL_mixer.Mix_FadeInMusic(music, 1, 2000);
	while(SDL_mixer.Mix_PlayingMusic() != 0) {
	    SDL.SDL_Delay(100);
	    timeleft --;
	    if(timeleft == 0)break;
	    if(timeleft == 25) {
		SDL_mixer.Mix_PlayChannel(-1, wave, 0);
	    }
	}
	SDL_mixer.Mix_FadeOutMusic(2000);
	SDL_mixer.Mix_FreeMusic(music);
	SDL_mixer.Mix_FreeChunk(wave);
	SDL.SDL_Delay(500);
	SDL_mixer.Mix_CloseAudio();
	SDL.SDL_Quit();
    }
}


