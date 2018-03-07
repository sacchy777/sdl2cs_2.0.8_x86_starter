using SDL2;

class TestVersion {
    public static void Main() {
	SDL.SDL_version compiled;
	SDL.SDL_version linked;
	SDL.SDL_VERSION(out compiled);
	System.Console.WriteLine("compiled version:{0}.{1}.{2}", compiled.major, compiled.minor, compiled.patch);
	SDL.SDL_GetVersion(out linked);
	System.Console.WriteLine("linked version:{0}.{1}.{2}.{3} ({4})", linked.major, linked.minor, linked.patch, SDL.SDL_GetRevisionNumber(), SDL.SDL_GetRevision());
	SDL.SDL_Quit();
    }
}
